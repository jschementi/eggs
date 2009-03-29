task :default => :rexap

SL  = 'C:\Program Files\Microsoft Silverlight\2.0.40115.0'
SL  = 'C:\Program Files\Microsoft Silverlight\3.0.40307.0' unless File.directory?(SL)
CSC = 'C:\Windows\Microsoft.NET\Framework\v3.5\csc.exe'
DLR = 'c:\dev\agdlr\bin\release'
#DLR = 'c:\dev\releases\agdlr-0.5.0\bin'
CHR = "#{DLR}\\Chiron.exe"

SYSTEM_REFERENCES = %W(mscorlib System System.Core System.Net System.Windows System.Windows.Browser)
DLR_REFERENCES    = %W(Microsoft.Scripting.Silverlight Microsoft.Scripting Microsoft.Scripting.Core Microsoft.Scripting.ExtensionAttribute IronRuby IronRuby.Libraries IronPython IronPython.Modules)

task :build do
  puts "Building lib/Eggs.dll"
  options = { :t => 'library', :out => 'lib/Eggs.dll', :nowarn => '1685', :debug => 'full' }
  flags = [:'nostdlib+', :noconfig, :'debug+']
  flags << :nologo unless ENV['DEBUG']
  csc 'src/*.cs', flags,
    convert_to_options(SYSTEM_REFERENCES, SL, 
      convert_to_options(DLR_REFERENCES, DLR, options))
end

task :xap => :build do
  puts "Generating eggs.xap"
  flags = ENV['DEBUG'] ? [] : [:silent]
  chr flags, :directory => 'lib', :zipdlr => 'eggs.xap'
end

task :clean do
  puts "Cleaning"
  require 'fileutils'
  FileUtils.rm 'lib/Eggs.dll' if File.exist?('lib/Eggs.dll')
  FileUtils.rm 'lib/Eggs.pdb' if File.exist?('lib/Eggs.pdb')
  FileUtils.rm 'eggs.xap' if File.exist?('eggs.xap')
end

task :rebuild => [:clean, :build]
task :rexap   => [:clean, :build, :xap]

def convert_to_options(assemblies, path, base_opts = {})
  assemblies.inject(base_opts) do |opts, ref|
    opts[:r] ||= []
    opts[:r] << "\"#{path.gsub('/', '\\')}\\#{ref}.dll\""
    opts
  end
end

def csc(files, flags, options)
  exe(CSC, files.gsub('/', '\\'), flags, options)
end

def chr(flags, options)
  exe(CHR, nil, flags, options)
end

def exe(cmd, args, flags, options)
  str_options = flags.inject("") {|flgs, flg| flgs << " /#{flg}"; flgs}
  str_options = options.inject(str_options) do |opts, (key,value)|
    [value].flatten.each do |v|
      opts << " /#{key}:#{v}"; opts
    end
    opts
  end
  cmd = "#{cmd}#{str_options} #{args}"
  puts cmd if ENV['DEBUG']
  system cmd
end
